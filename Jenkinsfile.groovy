@Library('jenkins-shared-library-groovy')_

import org.sample.jenkins.CSharpBuilder

CSharpBuilder builder = new CSharpBuilder(this)

builder.run()
